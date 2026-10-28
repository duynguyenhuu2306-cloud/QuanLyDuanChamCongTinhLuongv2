/* Additional CRUD and workflow screens for the weekly project checklist. */
let taskPresentation = 'kanban';

const loadDataFromCore = loadDataFor;
loadDataFor = async function (page = currentPage) {
  await loadDataFromCore(page);
  if (!state.token || state.loading || !canAccessPage(page)) return;

  const jobs = [];
  if (needsRole('admin', 'pm') && !state.usersLoaded && ['dashboard', 'tasks', 'reports', 'people'].includes(page)) {
    jobs.push(api(apiPaths.users).then(rows => {
      state.users = normalizeArray(rows).map(userDTO);
      state.usersLoaded = true;
    }));
  }
  if (needsRole('employee') && ['dashboard', 'attendance', 'tasks'].includes(page)) {
    if (!state.tasksLoaded) jobs.push(api(`${apiPaths.tasks}/all`).then(rows => {
      state.tasks = normalizeArray(rows).map(taskDTO);
      state.tasksLoaded = true;
    }));
    if (!state.assignmentsLoaded) jobs.push(api(`${apiPaths.assignments}/all`).then(rows => {
      state.assignments = normalizeArray(rows);
      state.assignmentsLoaded = true;
    }));
  }
  if (needsRole('employee') && page === 'payroll' && !state.payrollLoaded) {
    jobs.push(api(`${apiPaths.payroll}/me`).then(rows => {
      state.payroll = normalizeArray(rows).map(payrollDTO);
      state.payrollLoaded = true;
    }));
  }
  if (needsRole('employee') && page === 'attendance' && !state.weekLoaded) {
    jobs.push(api(`${apiPaths.attendance}/week`).then(data => {
      state.weekSummary = data;
      state.weekLoaded = true;
    }));
  }
  if (!jobs.length) return;
  try {
    await Promise.all(jobs);
    hydrateSampleLists();
    render();
  } catch (error) {
    state.apiError = error.message;
    render();
  }
};

function assignmentForTask(taskId) {
  return state.assignments.filter(a => (a.Macongviec || a.macongviec) === taskId);
}

function employeeDashboard() {
  const now = new Date();
  const monday = new Date(now.getFullYear(), now.getMonth(), now.getDate() - ((now.getDay() + 6) % 7));
  const nextMonday = new Date(monday); nextMonday.setDate(monday.getDate() + 7);
  const weekRows = attendance.filter(row => { const day = new Date(`${row.date}T00:00:00`); return day >= monday && day < nextMonday; });
  const weekHours = weekRows.reduce((sum, row) => sum + Number(row.hours || 0), 0);
  const pendingTimesheets = attendance.filter(row => row.status === 'Pending').length;
  const pendingRequests = leaveRequests.filter(row => row.status === 'Pending').length;
  return `${heading(`Xin chào, ${escapeHTML(state.username || 'bạn')} 👋`, 'Không gian làm việc cá nhân của bạn.')}
    <div class="metrics">
      ${metric('☷', 'purple', 'Công việc được giao', tasks.length, `${tasks.filter(task => task.status !== 'Done').length} việc chưa hoàn tất`)}
      ${metric('◷', 'green', 'Giờ công tuần này', `${weekHours} giờ`, `${weekRows.length} bản ghi trong tuần`)}
      ${metric('✓', 'orange', 'Chấm công chờ duyệt', pendingTimesheets, 'Bản ghi của bạn')}
      ${metric('▧', 'blue', 'Đơn nghỉ / OT chờ duyệt', pendingRequests, 'Yêu cầu của bạn')}
    </div>
    <div class="dashboard-grid">
      <article class="panel"><div class="panel-header"><div><h2 class="panel-title">Công việc của tôi</h2><div class="panel-subtitle">Các công việc đã được giao cho bạn</div></div><button class="text-link" data-page="tasks">Tất cả công việc →</button></div>${taskTable(tasks.slice(0, 5))}</article>
      <article class="panel"><div class="panel-header"><div><h2 class="panel-title">Chấm công gần đây</h2><div class="panel-subtitle">Chỉ hiển thị bản ghi của tài khoản này</div></div><button class="text-link" data-page="attendance">Mở chấm công →</button></div>${attendanceTable(attendance.slice(0, 5))}</article>
    </div>
    <div class="panel dashboard-shortcuts"><div class="panel-header"><div><h2 class="panel-title">Truy cập nhanh</h2><div class="panel-subtitle">Các chức năng cá nhân</div></div></div><div class="shortcut-actions"><button class="btn btn-quiet" data-page="leave">Nghỉ phép &amp; OT</button><button class="btn btn-quiet" data-page="payroll">Phiếu lương của tôi</button><button class="btn btn-primary" data-action="new-attendance">＋ Ghi nhận giờ</button></div></div>`;
}

function accountantDashboard() {
  const netTotal = payroll.reduce((sum, record) => sum + Number(record.net || 0), 0);
  const draftRuns = state.payRuns.filter(run => run.status === 'Draft').length;
  return `${heading(`Xin chào, ${escapeHTML(state.username || 'bạn')} 👋`, 'Tổng quan nghiệp vụ tiền lương và dữ liệu chấm công.')}
    <div class="metrics metrics-three">
      ${metric('＄', 'purple', 'Tổng thực nhận', money(netTotal), `${payroll.length} phiếu lương`)}
      ${metric('▤', 'green', 'Kỳ lương', state.payRuns.length, `${draftRuns} kỳ đang lập`)}
      ${metric('◷', 'orange', 'Bản ghi chấm công', attendance.length, 'Dữ liệu phục vụ tính lương')}
    </div>
    <div class="dashboard-grid">
      <article class="panel"><div class="panel-header"><div><h2 class="panel-title">Phiếu lương gần đây</h2><div class="panel-subtitle">Tổng hợp theo dữ liệu API</div></div><button class="text-link" data-page="payroll">Mở bảng lương →</button></div>${payroll.slice(0, 6).map(record => `<div class="approval-card"><span class="approval-initial">＄</span><div class="approval-details"><b>${escapeHTML(joinName(record.userId))}</b><small>${escapeHTML(record.id)} · ${money(record.net)}</small></div></div>`).join('') || '<div class="empty-state">Chưa có phiếu lương.</div>'}</article>
      <article class="panel"><div class="panel-header"><div><h2 class="panel-title">Chấm công</h2><div class="panel-subtitle">Bản ghi dùng để tổng hợp lương</div></div><button class="text-link" data-page="attendance">Xem chấm công →</button></div><p class="panel-subtitle">Hệ thống đã tải ${attendance.length} bản ghi chấm công.</p></article>
    </div>`;
}

function managerDashboard() {
  const pendingAttendance = attendance.filter(record => record.status === 'Pending').length;
  const pendingLeave = leaveRequests.filter(record => record.status === 'Pending').length;
  const metrics = [
    metric('▦', 'purple', 'Dự án', state.projectsLoaded ? state.projects.length : '—', 'Dữ liệu quản lý dự án'),
    metric('☷', 'green', 'Công việc chưa hoàn tất', state.tasksLoaded ? state.tasks.filter(task => task.status !== 'Done').length : '—', `${tasks.length} công việc tải từ API`),
    metric('◷', 'orange', 'Chấm công chờ duyệt', state.attendanceLoaded ? pendingAttendance : '—', 'Cần xem xét'),
    needsRole('admin') ? metric('＄', 'blue', 'Lương thực nhận', state.payrollLoaded ? money(payroll.reduce((sum, record) => sum + record.net, 0)) : '—', 'Chỉ dành cho quản trị viên') : ''
  ].join('');
  return `${heading(`Xin chào, ${escapeHTML(state.username || 'bạn')} 👋`, 'Tổng quan quản lý công việc và nhân sự.', '<button class="btn btn-primary" data-action="new-task">＋ Tạo công việc</button>')}
    <div class="metrics ${needsRole('admin') ? '' : 'metrics-three'}">${metrics}</div>
    <div class="dashboard-grid">
      <article class="panel"><div class="panel-header"><div><h2 class="panel-title">Dự án</h2><div class="panel-subtitle">Dữ liệu từ API dự án</div></div><button class="text-link" data-page="projects">Xem dự án →</button></div>${projects.length ? `<div class="project-list">${projectRows()}</div>` : '<div class="empty-state">Chưa có dự án để hiển thị.</div>'}</article>
      <article class="panel"><div class="panel-header"><div><h2 class="panel-title">Yêu cầu chờ xử lý</h2><div class="panel-subtitle">Chấm công và nghỉ phép / OT</div></div><button class="text-link" data-page="attendance">Mở chấm công →</button></div><div class="legend-list"><div class="legend-row"><span>Chấm công</span><b>${state.attendanceLoaded ? pendingAttendance : '—'}</b></div><div class="legend-row"><span>Nghỉ phép / OT</span><b>${state.leaveLoaded ? pendingLeave : '—'}</b></div></div></article>
    </div>
    <article class="panel table-panel"><div class="panel-header"><div><h2 class="panel-title">Công việc gần đây</h2><div class="panel-subtitle">Theo dõi tiến độ thực hiện</div></div><button class="text-link" data-page="tasks">Xem công việc →</button></div>${taskTable(tasks.slice(0, 6))}</article>
    ${needsRole('admin') ? `<article class="panel"><div class="panel-header"><div><h2 class="panel-title">Nhân sự</h2><div class="panel-subtitle">${state.usersLoaded ? `${state.users.length} tài khoản trong hệ thống` : 'Danh sách tài khoản'}</div></div><button class="text-link" data-page="people">Quản lý nhân sự →</button></div></article>` : ''}`;
}

dashboardLive = function () {
  if (needsRole('employee') && !needsRole('admin', 'pm', 'accountant')) return employeeDashboard();
  if (needsRole('accountant') && !needsRole('admin', 'pm')) return accountantDashboard();
  return managerDashboard();
};

tasksPage = function () {
  const filtered = tasks.filter(t =>
    (taskFilter === 'all' || t.status === taskFilter) &&
    (listFilters.taskProject === 'all' || t.projectId === listFilters.taskProject) &&
    (listFilters.taskPriority === 'all' || t.priority === listFilters.taskPriority) &&
    `${t.title} ${t.project} ${t.assignee}`.toLowerCase().includes(globalQuery.toLowerCase()));
  const page = pageSlice(filtered, 'tasks');
  const manager = needsRole('admin', 'pm');
  const controls = `<div class="toolbar"><input id="pageSearch" placeholder="⌕  Tìm công việc..." value="${escapeHTML(globalQuery)}"><select id="taskProject"><option value="all">Tất cả dự án</option>${projects.map(p => `<option value="${escapeHTML(p.id)}">${escapeHTML(p.name)}</option>`).join('')}</select><select id="taskPriority"><option value="all">Mọi mức ưu tiên</option><option value="High">Cao</option><option value="Medium">Trung bình</option><option value="Low">Thấp</option></select><div class="view-switch"><button data-task-view="kanban" class="${taskPresentation === 'kanban' ? 'active' : ''}">Kanban</button><button data-task-view="list" class="${taskPresentation === 'list' ? 'active' : ''}">Danh sách</button></div></div>`;
  const board = ['Todo', 'InProgress', 'Done'].map(status => {
    const lane = page.rows.filter(t => t.status === status);
    return `<section class="kanban-column"><header><span>${statusText(status)}</span><b>${lane.length}</b></header>${lane.map(t => {
      const assigned = assignmentForTask(t.id);
      return `<article class="kanban-card"><div class="kanban-card-top"><span class="priority priority-${t.priority.toLowerCase()}">◆ ${t.priority === 'High' ? 'Cao' : t.priority === 'Medium' ? 'Trung bình' : 'Thấp'}</span>${manager ? `<button class="icon-button" title="Sửa công việc" data-action="edit-task" data-id="${escapeHTML(t.id)}">✎</button>` : ''}</div><h3>${escapeHTML(t.title)}</h3><p>${escapeHTML(t.project || t.projectId || 'Chưa có dự án')}</p><small>${t.due ? `Hạn ${dateVN(t.due)}` : 'Chưa đặt hạn'}</small>${manager ? `<label class="kanban-status-label">Trạng thái<select data-task-status="${escapeHTML(t.id)}"><option value="Todo" ${t.status === 'Todo' ? 'selected' : ''}>Cần làm</option><option value="InProgress" ${t.status === 'InProgress' ? 'selected' : ''}>Đang thực hiện</option><option value="Done" ${t.status === 'Done' ? 'selected' : ''}>Hoàn thành</option></select></label><div class="assignment-list">${assigned.map(a => `<span>${escapeHTML(joinName(a.Manguoidung || a.manguoidung))}<button title="Gỡ phân công" data-action="remove-assignment" data-id="${escapeHTML(a.Ma_phancong || a.ma_phancong)}">×</button></span>`).join('')}<select data-assign-task="${escapeHTML(t.id)}"><option value="">＋ Phân công</option>${state.users.filter(u => u.role === 'Employee' && u.active).map(u => `<option value="${escapeHTML(u.id)}">${escapeHTML(u.name)}</option>`).join('')}</select></div><button class="text-link danger-link" data-action="delete-task" data-id="${escapeHTML(t.id)}">Xóa công việc</button>` : `<span class="task-assignee">${escapeHTML(t.assignee)}</span>`}</article>`;
    }).join('') || '<div class="kanban-empty">Không có công việc</div>'}</section>`;
  }).join('');
  return `${heading('Công việc', 'Bảng Kanban và dữ liệu công việc được tải từ API.', manager ? '<button class="btn btn-primary" data-action="new-task">＋ Tạo công việc</button>' : '')}${pageTabs([['all', 'Tất cả'], ['InProgress', 'Đang thực hiện'], ['Todo', 'Cần làm'], ['Done', 'Hoàn thành']], taskFilter)}${controls}${taskPresentation === 'kanban' ? `<div class="kanban-board">${board}</div>${listFooter(page, 'tasks', 'công việc')}` : `<article class="panel">${taskTable(page.rows)}${listFooter(page, 'tasks', 'công việc')}</article>`}`;
};

projectsPage = function () {
  const filtered = projects.filter(p => (projectFilter === 'all' || p.status === projectFilter) && `${p.name} ${p.id} ${p.owner}`.toLowerCase().includes(globalQuery.toLowerCase()));
  const page = pageSlice(filtered, 'projects');
  return `${heading('Dự án', 'Dự án lấy từ CSDL qua API.', needsRole('admin', 'pm') ? '<button class="btn btn-primary" data-action="new-project">＋ Tạo dự án</button>' : '')}${pageTabs([['all', 'Tất cả dự án'], ['InProgress', 'Đang thực hiện'], ['Planning', 'Lên kế hoạch'], ['Completed', 'Hoàn thành']], projectFilter)}<div class="toolbar"><input id="pageSearch" placeholder="⌕  Tìm dự án..." value="${escapeHTML(globalQuery)}"><select id="projectStatus"><option value="all">Tất cả trạng thái</option><option value="InProgress">Đang thực hiện</option><option value="Planning">Lên kế hoạch</option><option value="Completed">Hoàn thành</option></select></div><article class="panel"><div class="table-overflow"><table class="data-table"><thead><tr><th>DỰ ÁN</th><th>QUẢN LÝ</th><th>NGÀY BẮT ĐẦU</th><th>HẠN DỰ KIẾN</th><th>TIẾN ĐỘ</th><th>TRẠNG THÁI</th><th></th></tr></thead><tbody>${page.rows.map(p => `<tr><td><b class="table-main">${escapeHTML(p.name)}</b><small class="table-sub">${escapeHTML(p.id)} · ${p.tasks} công việc</small></td><td>${escapeHTML(p.owner)}</td><td>${p.start ? dateVN(p.start) : '—'}</td><td>${p.end ? dateVN(p.end) : '—'}</td><td>${p.progress}%</td><td>${badge(p.status)}</td><td>${needsRole('admin', 'pm') ? `<button class="text-link" data-action="edit-project" data-id="${escapeHTML(p.id)}">Sửa</button> <button class="text-link danger-link" data-action="delete-project" data-id="${escapeHTML(p.id)}">Xóa</button>` : '—'}</td></tr>`).join('') || '<tr><td colspan="7" class="empty-state">Không có dự án phù hợp.</td></tr>'}</tbody></table></div>${listFooter(page, 'projects', 'dự án')}</article>`;
};

attendancePage = function () {
  const visibleAttendance = needsRole('employee') && !needsRole('admin', 'pm', 'accountant') ? attendance.filter(isMyAttendance) : attendance;
  const filtered = visibleAttendance.filter(a => (attendanceFilter === 'all' || a.status === attendanceFilter) && (listFilters.attendanceMonth === 'all' || a.date?.slice(0, 7) === listFilters.attendanceMonth) && `${a.name} ${a.task} ${a.id}`.toLowerCase().includes(globalQuery.toLowerCase()));
  const page = pageSlice(filtered, 'attendance');
  const months = [...new Set(attendance.map(a => a.date?.slice(0, 7)).filter(Boolean))].sort().reverse();
  const week = state.weekSummary;
  return `${heading('Chấm công', 'Ghi giờ theo ngày, theo dõi tổng giờ tuần và gửi PM duyệt.', needsRole('employee') ? '<button class="btn btn-primary" data-action="new-attendance">＋ Ghi nhận giờ</button>' : '')}${needsRole('employee') && week ? `<article class="panel week-summary"><div><small>TỔNG GIỜ TUẦN</small><strong>${Number(week.tongGio || 0)} giờ</strong></div><span>${week.tuNgay ? dateVN(dateOnly(week.tuNgay)) : ''} – ${week.denNgay ? dateVN(dateOnly(week.denNgay)) : ''}</span></article>` : ''}${pageTabs([['all', 'Tất cả bản ghi'], ['Pending', 'Chờ duyệt'], ['Approved', 'Đã duyệt'], ['Rejected', 'Từ chối']], attendanceFilter)}<div class="toolbar"><input id="pageSearch" placeholder="⌕  Tìm nhân viên hoặc công việc..." value="${escapeHTML(globalQuery)}"><select id="attendanceMonth"><option value="all">Tất cả tháng</option>${months.map(m => `<option value="${m}">${new Date(m + '-01T00:00:00').toLocaleDateString('vi-VN', { month: 'long', year: 'numeric' })}</option>`).join('')}</select></div><article class="panel">${attendanceTable(page.rows)}${listFooter(page, 'attendance', 'bản ghi')}</article>`;
};

attendanceTable = function (rows) {
  return `<div class="table-overflow"><table class="data-table"><thead><tr><th>MÃ CHẤM CÔNG</th><th>NHÂN VIÊN</th><th>CÔNG VIỆC</th><th>NGÀY LÀM</th><th>SỐ GIỜ</th><th>TRẠNG THÁI</th><th>THAO TÁC</th></tr></thead><tbody>${rows.map(a => {
    const editable = needsRole('employee') && (a.status === 'Pending' || a.status === 'Rejected');
    const reviewable = needsRole('admin', 'pm') && a.status === 'Pending';
    return `<tr><td><b class="table-main">${escapeHTML(a.id)}</b></td><td>${escapeHTML(a.name)}</td><td>${escapeHTML(a.task)}</td><td>${a.date ? dateVN(a.date) : '—'}</td><td>${a.hours} giờ</td><td>${badge(a.status)}</td><td>${editable ? `<button class="text-link" data-action="edit-timesheet" data-id="${escapeHTML(a.id)}">Sửa</button> <button class="text-link danger-link" data-action="delete-timesheet" data-id="${escapeHTML(a.id)}">Xóa</button>` : reviewable ? `<button class="text-link" data-action="review-attendance" data-id="${escapeHTML(a.id)}">Duyệt</button>` : '—'}</td></tr>`;
  }).join('') || '<tr><td colspan="7" class="empty-state">Không có bản ghi chấm công phù hợp.</td></tr>'}</tbody></table></div>`;
};

leavePage = function () {
  const rows = leaveRequests.filter(a => (leaveFilter === 'all' || (leaveFilter === 'Pending' && a.status === 'Pending') || (leaveFilter === 'Leave' && a.type === 'Leave') || (leaveFilter === 'OT' && a.type === 'OT')) && `${a.name} ${a.type} ${a.id}`.toLowerCase().includes(globalQuery.toLowerCase()));
  const page = pageSlice(rows, 'leave');
  const employee = needsRole('employee');
  const reviewer = needsRole('admin', 'pm');
  if (!employee && !reviewer) return `${heading('Nghỉ phép & OT', 'Tài khoản hiện tại không có quyền xem module này.')}`;
  return `${heading('Nghỉ phép & OT', 'Gửi yêu cầu nghỉ hoặc OT, theo dõi trạng thái duyệt và xuất báo cáo CSV.', employee ? '<button class="btn btn-primary" data-action="new-leave">＋ Tạo yêu cầu</button>' : '<button class="btn btn-quiet" data-action="export">⇩ Xuất CSV</button>')}${pageTabs([['all', 'Tất cả yêu cầu'], ['Pending', 'Chờ duyệt'], ['Leave', 'Nghỉ phép'], ['OT', 'Làm thêm giờ']], leaveFilter)}<div class="toolbar"><input id="pageSearch" placeholder="⌕  Tìm nhân viên hoặc mã đơn..." value="${escapeHTML(globalQuery)}"></div><article class="panel"><div class="table-overflow"><table class="data-table"><thead><tr><th>MÃ ĐƠN</th><th>NHÂN VIÊN</th><th>LOẠI</th><th>THỜI GIAN</th><th>SỐ GIỜ OT</th><th>LÝ DO</th><th>TRẠNG THÁI</th><th>THAO TÁC</th></tr></thead><tbody>${page.rows.map(r => {
    const editable = employee && (r.status === 'Pending' || r.status === 'Rejected');
    const reviewable = needsRole('admin', 'pm') && r.status === 'Pending';
    return `<tr><td><b class="table-main">${escapeHTML(r.id)}</b></td><td>${escapeHTML(r.name)}</td><td>${r.type === 'OT' ? 'Làm thêm' : 'Nghỉ phép'}</td><td>${dateVN(r.start)}${r.start !== r.end ? ` – ${dateVN(r.end)}` : ''}</td><td>${r.type === 'OT' ? `${r.hours} giờ` : '—'}</td><td>${escapeHTML(r.reason || '—')}${r.rejectReason ? `<small class="table-sub">Lý do từ chối: ${escapeHTML(r.rejectReason)}</small>` : ''}</td><td>${badge(r.status)}</td><td>${editable ? `<button class="text-link" data-action="edit-leave" data-id="${escapeHTML(r.id)}">Sửa</button> <button class="text-link danger-link" data-action="delete-leave" data-id="${escapeHTML(r.id)}">Xóa</button>` : reviewable ? `<button class="text-link" data-action="review-leave" data-id="${escapeHTML(r.id)}">Duyệt</button>` : '—'}</td></tr>`;
  }).join('') || '<tr><td colspan="8" class="empty-state">Không có yêu cầu phù hợp.</td></tr>'}</tbody></table></div>${listFooter(page, 'leave', 'yêu cầu')}</article>`;
};

payrollPage = function () {
  const employee = needsRole('employee');
  const payrollManager = needsRole('admin', 'accountant');
  if (!employee && !payrollManager) return `${heading('Bảng lương', 'Tài khoản hiện tại không có quyền xem module bảng lương.')}`;
  const tab = employee ? 'all' : (window.payrollTab || 'all');
  const allRows = payroll.filter(p => `${p.name} ${p.id} ${p.period}`.toLowerCase().includes(globalQuery.toLowerCase()));
  const list = tab === 'runs' ? state.payRuns : tab === 'policies' ? state.policies : allRows;
  const page = pageSlice(list, `payroll_${tab}`);
  const get = (obj, lower, upper) => obj?.[lower] ?? obj?.[upper];
  const table = tab === 'runs' ? `<table class="data-table"><thead><tr><th>KỲ LƯƠNG</th><th>THỜI GIAN</th><th>TRẠNG THÁI</th><th>THAO TÁC</th></tr></thead><tbody>${page.rows.map(r => `<tr><td><b class="table-main">${escapeHTML(r.id)}</b></td><td>${r.start ? dateVN(r.start) : '—'} – ${r.end ? dateVN(r.end) : '—'}</td><td>${badge(r.status)}</td><td><div class="row-actions">${r.status === 'Draft' ? `<button class="text-link" data-action="generate-payroll" data-id="${escapeHTML(r.id)}">Tính lương</button><button class="text-link" data-action="complete-payroll-run" data-id="${escapeHTML(r.id)}">Hoàn tất</button>` : '—'}</div></td></tr>`).join('') || '<tr><td colspan="4" class="empty-state">Chưa có kỳ lương.</td></tr>'}</tbody></table>` : tab === 'policies' ? `<table class="data-table"><thead><tr><th>MÃ CHÍNH SÁCH</th><th>ĐƠN GIÁ / GIỜ</th><th>HỆ SỐ OT</th><th>QUY TẮC LÀM TRÒN</th><th>HIỆU LỰC</th><th></th></tr></thead><tbody>${page.rows.map(p => { const id = get(p, 'machinhsach', 'Machinhsach'); return `<tr><td>${escapeHTML(id)}</td><td>${money(Number(get(p, 'luonggiocoban', 'Luonggiocoban') || 0))}</td><td>${escapeHTML(get(p, 'hesonhanlamthem', 'Hesonhanlamthem'))}×</td><td>${escapeHTML(get(p, 'quytaclamtron', 'Quytaclamtron') || 'Làm tròn đến đồng')}</td><td>${dateVN(dateOnly(get(p, 'hieulucngay', 'Hieulucngay')))}</td><td><div class="row-actions"><button class="text-link" data-action="edit-policy" data-id="${escapeHTML(id)}">Sửa</button><button class="text-link danger-link" data-action="delete-policy" data-id="${escapeHTML(id)}">Xóa</button></div></td></tr>`; }).join('') || '<tr><td colspan="6" class="empty-state">Chưa có chính sách lương.</td></tr>'}</tbody></table>` : `<table class="data-table"><thead><tr><th>MÃ PHIẾU</th><th>NHÂN VIÊN</th><th>KỲ</th><th>GIỜ THƯỜNG</th><th>GIỜ OT</th><th>TỔNG LƯƠNG</th><th>KHẤU TRỪ</th><th>THỰC NHẬN</th><th></th></tr></thead><tbody>${page.rows.map(p => `<tr><td><b class="table-main">${escapeHTML(p.id)}</b></td><td>${escapeHTML(joinName(p.userId))}</td><td>${escapeHTML(p.period || p.runId)}</td><td>${p.hours}</td><td>${p.ot}</td><td>${money(p.gross)}</td><td>${money(p.deduction)}</td><td><b class="table-main">${money(p.net)}</b></td><td><button class="text-link" data-action="payslip-pdf" data-id="${escapeHTML(p.id)}">In / PDF</button></td></tr>`).join('') || '<tr><td colspan="9" class="empty-state">Không có phiếu lương.</td></tr>'}</tbody></table>`;
  const actions = payrollManager ? `<button class="btn btn-quiet" data-action="new-policy">＋ Chính sách</button><button class="btn btn-primary" data-action="new-payroll">＋ Tạo kỳ lương</button>` : '';
  return `${heading('Bảng lương', 'Tính lương từ chấm công và OT đã duyệt theo chính sách có hiệu lực.', actions)}${payrollManager ? pageTabs([['all', 'Phiếu lương'], ['runs', 'Kỳ lương'], ['policies', 'Chính sách lương']], tab) : ''}${tab === 'all' ? `<div class="metrics">${metric('＄', 'purple', 'Tổng thực nhận', money(allRows.reduce((sum, p) => sum + p.net, 0)), `${allRows.length} phiếu lương`)}</div><div class="toolbar"><input id="pageSearch" placeholder="⌕  Tìm nhân viên hoặc mã phiếu..." value="${escapeHTML(globalQuery)}"></div>` : ''}<article class="panel"><div class="table-overflow">${table}</div>${listFooter(page, `payroll_${tab}`, tab === 'runs' ? 'kỳ lương' : tab === 'policies' ? 'chính sách' : 'phiếu lương')}</article>`;
};

const openCreateFromCore = openCreate;
openCreate = function (kind, id) {
  if (kind === 'edit-timesheet') {
    const a = attendance.find(x => x.id === id); if (!a) return;
    if (needsRole('employee') && !isMyAttendance(a)) return toast('Bạn chỉ có thể chỉnh sửa bảng chấm công của chính mình.');
    currentModalAction = kind; window.editingTimesheetId = id;
    modal('Chỉnh sửa bảng chấm công', [{ name: 'task', label: 'Công việc', options: tasks.map(t => [t.id, t.title]) }, { name: 'date', label: 'Ngày làm việc', type: 'date', required: true, value: a.date }, { name: 'hours', label: 'Số giờ', type: 'number', required: true, value: a.hours }]);
    $('#modalForm select[name="task"]').value = a.taskId; return;
  }
  if (kind === 'edit-leave') {
    const a = leaveRequests.find(x => x.id === id); if (!a) return;
    currentModalAction = kind; window.editingLeaveId = id;
    modal('Chỉnh sửa yêu cầu nghỉ / OT', [{ name: 'type', label: 'Loại', options: [['Leave', 'Nghỉ phép'], ['OT', 'Làm thêm giờ']] }, { name: 'date', label: 'Ngày bắt đầu', type: 'date', required: true, value: a.start }, { name: 'end', label: 'Ngày kết thúc', type: 'date', required: true, value: a.end }, { name: 'hours', label: 'Số giờ OT', type: 'number', value: a.type === 'OT' ? a.hours : '' }, { name: 'reason', label: 'Lý do', area: true, full: true, required: true, value: a.reason }]);
    $('#modalForm select[name="type"]').value = a.type; $('#modalForm textarea[name="reason"]').value = a.reason || ''; return;
  }
  if (kind === 'new-policy' || kind === 'edit-policy') {
    const p = kind === 'edit-policy' ? state.policies.find(x => (x.machinhsach || x.Machinhsach) === id) : null;
    if (kind === 'edit-policy' && !p) return;
    currentModalAction = kind; window.editingPolicyId = id || '';
    modal(kind === 'new-policy' ? 'Tạo chính sách lương' : 'Cập nhật chính sách lương', [
      { name: 'id', label: 'Mã chính sách', required: true, value: p?.machinhsach || p?.Machinhsach || '' },
      { name: 'rate', label: 'Đơn giá giờ cơ bản (₫)', type: 'number', required: true, value: p?.luonggiocoban || p?.Luonggiocoban || '' },
      { name: 'ot', label: 'Hệ số làm thêm', type: 'number', required: true, value: p?.hesonhanlamthem || p?.Hesonhanlamthem || '1.5' },
      { name: 'rounding', label: 'Quy tắc làm tròn giờ', options: [['Làm tròn 1 giờ', 'Làm tròn theo giờ'], ['Làm tròn 30 phút', 'Làm tròn theo 30 phút'], ['Làm tròn 15 phút', 'Làm tròn theo 15 phút']] },
      { name: 'effective', label: 'Ngày hiệu lực', type: 'date', required: true, value: dateOnly(p?.hieulucngay || p?.Hieulucngay) }
    ]);
    if (kind === 'edit-policy') $('#modalForm select[name="rounding"]').value = p?.quytaclamtron || p?.Quytaclamtron || 'Làm tròn 1 giờ';
    if (kind === 'edit-policy') $('#modalForm input[name="id"]').readOnly = true;
    return;
  }
  if (kind === 'edit-project') {
    const p = projects.find(x => x.id === id); if (!p) return;
    currentModalAction = 'edit-project'; window.editingProjectId = id;
    modal('Cập nhật dự án', [{ name: 'name', label: 'Tên dự án', required: true, value: p.name }, { name: 'status', label: 'Trạng thái', options: [['Planning', 'Lên kế hoạch'], ['InProgress', 'Đang thực hiện'], ['OnHold', 'Tạm dừng'], ['Completed', 'Hoàn thành']] }, { name: 'start', label: 'Ngày bắt đầu', type: 'date', required: true, value: p.start }, { name: 'end', label: 'Ngày kết thúc', type: 'date', value: p.end }, { name: 'description', label: 'Mô tả', area: true, full: true, value: p.description }]);
    $('#modalForm select[name="status"]').value = p.status; $('#modalForm textarea[name="description"]').value = p.description || ''; return;
  }
  if (kind === 'edit-task') {
    const t = state.tasks.find(x => x.id === id); if (!t) return;
    currentModalAction = 'edit-task'; window.editingTaskId = id;
    modal('Cập nhật công việc', [{ name: 'title', label: 'Tiêu đề', required: true, value: t.title }, { name: 'project', label: 'Dự án', options: projects.map(p => [p.id, p.name]) }, { name: 'status', label: 'Trạng thái', options: [['Todo', 'Cần làm'], ['InProgress', 'Đang thực hiện'], ['Done', 'Hoàn thành']] }, { name: 'priority', label: 'Ưu tiên', options: [['High', 'Cao'], ['Medium', 'Trung bình'], ['Low', 'Thấp']] }, { name: 'start', label: 'Ngày bắt đầu', type: 'date', value: t.start }, { name: 'due', label: 'Hạn chót', type: 'date', value: t.due }, { name: 'description', label: 'Mô tả', area: true, full: true, value: t.description }]);
    $('#modalForm select[name="project"]').value = t.projectId; $('#modalForm select[name="status"]').value = t.status; $('#modalForm select[name="priority"]').value = t.priority; $('#modalForm textarea[name="description"]').value = t.description || ''; return;
  }
  openCreateFromCore(kind);
};

const submitModalFromCore = submitModal;
submitModal = async function (fd) {
  const v = key => String(fd.get(key) || '').trim();
  if (currentModalAction === 'edit-project') {
    const p = state.projects.find(x => x.id === window.editingProjectId);
    await api(`${apiPaths.projects}/${encodeURIComponent(window.editingProjectId)}`, { method: 'PUT', body: JSON.stringify({ Maduan: window.editingProjectId, Tenduan: v('name'), Motada: v('description'), Ngaybatdau: v('start'), Ngayketthuc: v('end') || null, Trangthai: v('status'), Nguoitao: p?.ownerId || state.userId }) });
    state.projectsLoaded = false; state.tasksLoaded = false; return;
  }
  if (currentModalAction === 'edit-timesheet') {
    const current = attendance.find(a => a.id === window.editingTimesheetId);
    if (!current || (needsRole('employee') && !isMyAttendance(current))) throw new Error('Bạn chỉ có thể chỉnh sửa bảng chấm công của chính mình.');
    await api(`${apiPaths.attendance}/${encodeURIComponent(window.editingTimesheetId)}`, { method: 'PUT', body: JSON.stringify({ Macongviec: v('task'), Ngaylamviec: v('date'), Sogiolam: Number(v('hours')) }) });
    state.attendanceLoaded = false; state.weekLoaded = false; return;
  }
  if (currentModalAction === 'edit-leave') {
    await api(`${apiPaths.leave}/${encodeURIComponent(window.editingLeaveId)}`, { method: 'PUT', body: JSON.stringify({ Loainghiphep: v('type'), Ngaybatdau: v('date'), Ngayketthuc: v('end'), Sogiolam: v('type') === 'OT' ? Number(v('hours')) : null, Lydo: v('reason') }) });
    state.leaveLoaded = false; return;
  }
  if (currentModalAction === 'new-policy' || currentModalAction === 'edit-policy') {
    const id = currentModalAction === 'edit-policy' ? window.editingPolicyId : v('id');
    const payload = { Machinhsach: id, Luonggiocoban: Number(v('rate')), Hesonhanlamthem: Number(v('ot')), Quytaclamtron: v('rounding'), Hieulucngay: v('effective') };
    await api(currentModalAction === 'new-policy' ? apiPaths.policies : `${apiPaths.policies}/${encodeURIComponent(id)}`, { method: currentModalAction === 'new-policy' ? 'POST' : 'PUT', body: JSON.stringify(payload) });
    state.policiesLoaded = false; return;
  }
  if (currentModalAction === 'edit-task') {
    const t = state.tasks.find(x => x.id === window.editingTaskId);
    await api(`${apiPaths.tasks}/${encodeURIComponent(window.editingTaskId)}`, { method: 'PUT', body: JSON.stringify({ Macongviec: window.editingTaskId, Maduan: v('project'), Tieude: v('title'), Mota: v('description'), Trangthai: v('status'), Mucdouutien: v('priority'), Ngaybatdau: v('start') || null, Dateline: v('due') || null, Nguoitao: t?.creator || state.userId }) });
    state.tasksLoaded = false; state.projectsLoaded = false; return;
  }
  return submitModalFromCore(fd);
};

function printPayslip(id) {
  const p = payroll.find(row => row.id === id); if (!p) return;
  const win = window.open('', '_blank', 'width=820,height=720');
  if (!win) { toast('Trình duyệt đang chặn cửa sổ in. Hãy cho phép popup cho website này.'); return; }
  win.document.write(`<!doctype html><html lang="vi"><head><meta charset="utf-8"><title>Phiếu lương ${escapeHTML(p.id)}</title><style>body{font:15px Arial,sans-serif;color:#1d2433;padding:48px;max-width:760px;margin:auto}h1{margin-bottom:4px}.muted{color:#68738b}.row{display:flex;justify-content:space-between;border-bottom:1px solid #e7eaf0;padding:14px 0}.total{font-size:22px;font-weight:bold;color:#5d50df}.print{margin-top:28px;padding:12px 18px;background:#6557e9;color:white;border:0;border-radius:8px;cursor:pointer}@media print{.print{display:none}body{padding:0}}</style></head><body><div class="muted">WORKLY · PHIẾU LƯƠNG</div><h1>${escapeHTML(joinName(p.userId))}</h1><p class="muted">Mã phiếu: ${escapeHTML(p.id)} · Kỳ lương: ${escapeHTML(p.period || '')}</p><div class="row"><span>Giờ làm thường</span><b>${p.hours} giờ</b></div><div class="row"><span>Giờ làm thêm</span><b>${p.ot} giờ</b></div><div class="row"><span>Tổng lương</span><b>${money(p.gross)}</b></div><div class="row"><span>Khoản trừ</span><b>${money(p.deduction)}</b></div><div class="row total"><span>Thực nhận</span><span>${money(p.net)}</span></div><button class="print" onclick="window.print()">In / Lưu PDF</button></body></html>`);
  win.document.close(); win.focus();
}

function refreshPayrollData() { state.payrollLoaded = false; state.payRunsLoaded = false; loadDataFor('payroll'); }

const bindDynamicFromCore = bindDynamic;
bindDynamic = function () {
  bindDynamicFromCore();
  document.querySelectorAll('[data-task-view]').forEach(button => button.onclick = () => { taskPresentation = button.dataset.taskView; render(); });
  document.querySelectorAll('[data-task-status]').forEach(select => select.onchange = async () => {
    const t = state.tasks.find(row => row.id === select.dataset.taskStatus); if (!t) return;
    select.disabled = true;
    try {
      await api(`${apiPaths.tasks}/${encodeURIComponent(t.id)}`, { method: 'PUT', body: JSON.stringify({ Macongviec: t.id, Maduan: t.projectId, Tieude: t.title, Mota: t.description, Trangthai: select.value, Mucdouutien: t.priority, Ngaybatdau: t.start || null, Dateline: t.due || null, Nguoitao: t.creator || state.userId }) });
      state.tasksLoaded = false; state.projectsLoaded = false; await loadDataFor('tasks'); toast('Đã cập nhật trạng thái công việc.');
    } catch (error) { toast(error.message); select.value = t.status; select.disabled = false; }
  });
  document.querySelectorAll('[data-assign-task]').forEach(select => select.onchange = async () => {
    if (!select.value) return;
    const exists = assignmentForTask(select.dataset.assignTask).some(a => (a.Manguoidung || a.manguoidung) === select.value);
    if (exists) { toast('Nhân viên đã được phân công vào công việc này.'); select.value = ''; return; }
    try {
      await api(apiPaths.assignments, { method: 'POST', body: JSON.stringify({ Ma_phancong: `PC${Date.now().toString(36).toUpperCase()}`, Macongviec: select.dataset.assignTask, Manguoidung: select.value }) });
      state.assignmentsLoaded = false; await loadDataFor('tasks'); toast('Đã phân công công việc.');
    } catch (error) { toast(error.message); }
  });
  document.querySelectorAll('[data-action="edit-project"], [data-action="edit-task"]').forEach(button => button.onclick = () => openCreate(button.dataset.action, button.dataset.id));
  document.querySelectorAll('[data-action="delete-project"]').forEach(button => button.onclick = async () => {
    if (!confirm('Xóa dự án này? Dự án có công việc liên quan có thể bị CSDL từ chối xóa.')) return;
    try { await api(`${apiPaths.projects}/${encodeURIComponent(button.dataset.id)}`, { method: 'DELETE' }); state.projectsLoaded = false; await loadDataFor('projects'); toast('Đã xóa dự án.'); } catch (error) { toast(error.message); }
  });
  document.querySelectorAll('[data-action="delete-task"]').forEach(button => button.onclick = async () => {
    if (!confirm('Xóa công việc này?')) return;
    try { await api(`${apiPaths.tasks}/${encodeURIComponent(button.dataset.id)}`, { method: 'DELETE' }); state.tasksLoaded = false; state.assignmentsLoaded = false; await loadDataFor('tasks'); toast('Đã xóa công việc.'); } catch (error) { toast(error.message); }
  });
  document.querySelectorAll('[data-action="remove-assignment"]').forEach(button => button.onclick = async () => {
    if (!confirm('Gỡ phân công này?')) return;
    try { await api(`${apiPaths.assignments}/${encodeURIComponent(button.dataset.id)}`, { method: 'DELETE' }); state.assignmentsLoaded = false; await loadDataFor('tasks'); toast('Đã gỡ phân công.'); } catch (error) { toast(error.message); }
  });
  document.querySelectorAll('[data-action="generate-payroll"]').forEach(button => button.onclick = async () => {
    button.disabled = true;
    try { const result = await api(`${apiPaths.payRuns}/${encodeURIComponent(button.dataset.id)}/generate`, { method: 'POST' }); toast(`Đã tạo ${result.created} phiếu lương.`); refreshPayrollData(); }
    catch (error) { toast(error.message); button.disabled = false; }
  });
  document.querySelectorAll('[data-action="complete-payroll-run"]').forEach(button => button.onclick = async () => {
    if (!confirm('Hoàn tất kỳ lương? Sau khi hoàn tất, kỳ sẽ không nhận thêm phiếu tính mới.')) return;
    try { await api(`${apiPaths.payRuns}/${encodeURIComponent(button.dataset.id)}/complete`, { method: 'PUT' }); toast('Đã hoàn tất kỳ lương.'); refreshPayrollData(); }
    catch (error) { toast(error.message); }
  });
  document.querySelectorAll('[data-action="payslip-pdf"]').forEach(button => button.onclick = () => printPayslip(button.dataset.id));
  document.querySelectorAll('[data-action="edit-policy"]').forEach(button => button.onclick = () => openCreate('edit-policy', button.dataset.id));
  document.querySelectorAll('[data-action="delete-policy"]').forEach(button => button.onclick = async () => {
    if (!confirm('Xóa chính sách lương này? Chính sách đang được phiếu lương sử dụng có thể bị CSDL từ chối xóa.')) return;
    try { await api(`${apiPaths.policies}/${encodeURIComponent(button.dataset.id)}`, { method: 'DELETE' }); state.policiesLoaded = false; await loadDataFor('payroll'); toast('Đã xóa chính sách.'); } catch (error) { toast(error.message); }
  });
  document.querySelectorAll('[data-action="edit-timesheet"], [data-action="edit-leave"]').forEach(button => button.onclick = () => openCreate(button.dataset.action, button.dataset.id));
  document.querySelectorAll('[data-action="delete-timesheet"]').forEach(button => button.onclick = async () => {
    const record = attendance.find(a => a.id === button.dataset.id);
    if (!record || (needsRole('employee') && !isMyAttendance(record))) return toast('Bạn chỉ có thể xóa bảng chấm công của chính mình.');
    if (!confirm('Xóa bảng chấm công này? Bản ghi sẽ được xóa mềm.')) return;
    try { await api(`${apiPaths.attendance}/${encodeURIComponent(button.dataset.id)}`, { method: 'DELETE' }); state.attendanceLoaded = false; state.weekLoaded = false; await loadDataFor('attendance'); toast('Đã xóa mềm bảng chấm công.'); } catch (error) { toast(error.message); }
  });
  document.querySelectorAll('[data-action="delete-leave"]').forEach(button => button.onclick = async () => {
    if (!confirm('Xóa yêu cầu này?')) return;
    try { await api(`${apiPaths.leave}/${encodeURIComponent(button.dataset.id)}`, { method: 'DELETE' }); state.leaveLoaded = false; await loadDataFor('leave'); toast('Đã xóa yêu cầu.'); } catch (error) { toast(error.message); }
  });
};

const renderFromCore = render;
render = function () { renderFromCore(); };
if (state.token) loadDataFor(currentPage);

/* Account controls and full user administration. */
const personRoleNames = { Admin: 'Quản trị viên', PM: 'Quản lý dự án', Accountant: 'Kế toán', Employee: 'Nhân viên' };
const personRoleOptions = [['Employee', 'Nhân viên'], ['PM', 'Quản lý dự án'], ['Accountant', 'Kế toán'], ['Admin', 'Quản trị viên']];

peoplePage = function () {
  const canManage = needsRole('admin');
  const filtered = people.filter(person =>
    (listFilters.peopleRole === 'all' || person.role.toLowerCase() === listFilters.peopleRole.toLowerCase()) &&
    (listFilters.peopleActive === 'all' || String(Boolean(person.active)) === listFilters.peopleActive) &&
    `${person.name} ${person.id} ${person.role} ${person.email} ${person.username}`.toLowerCase().includes(globalQuery.toLowerCase()));
  const page = pageSlice(filtered, 'people');
  const rows = page.rows.map(person => `<tr>
    <td><span class="person-line">${avatar(escapeHTML(person.initials || initials(person.name || person.username || 'NV')), person.color)}<b>${escapeHTML(person.name || person.username)}</b></span><small class="table-sub">${escapeHTML(person.username || '')}</small></td>
    <td>${escapeHTML(person.id)}</td>
    <td>${escapeHTML(person.email || '—')}</td>
    <td>${escapeHTML(personRoleNames[person.role] || person.role || '—')}</td>
    <td><span class="status-pill ${person.active ? '' : 'status-pending'}">${person.active ? 'Đang hoạt động' : 'Đã khóa'}</span></td>
    ${canManage ? `<td><div class="row-actions"><button class="text-link" data-action="edit-person" data-id="${escapeHTML(person.id)}">Sửa</button><button class="text-link danger-link" data-action="delete-person" data-id="${escapeHTML(person.id)}">Xóa</button></div></td>` : ''}
  </tr>`).join('');
  return `${heading('Nhân sự', 'Quản lý tài khoản nhân sự từ cơ sở dữ liệu qua API.', canManage ? '<button class="btn btn-primary" data-action="new-person">＋ Thêm nhân sự</button>' : '')}
    <div class="toolbar"><input id="pageSearch" placeholder="⌕  Tìm nhân viên..." value="${escapeHTML(globalQuery)}"><select id="peopleRole"><option value="all">Tất cả vai trò</option>${[...new Set(people.map(person => person.role).filter(Boolean))].map(role => `<option value="${escapeHTML(role)}">${escapeHTML(personRoleNames[role] || role)}</option>`).join('')}</select><select id="peopleActive"><option value="all">Mọi trạng thái</option><option value="true">Đang hoạt động</option><option value="false">Đã khóa</option></select></div>
    <article class="panel"><div class="table-overflow"><table class="data-table"><thead><tr><th>NHÂN VIÊN</th><th>MÃ NGƯỜI DÙNG</th><th>EMAIL</th><th>VAI TRÒ</th><th>TRẠNG THÁI</th>${canManage ? '<th>THAO TÁC</th>' : ''}</tr></thead><tbody>${rows || `<tr><td colspan="${canManage ? 6 : 5}" class="empty-state">Không có dữ liệu nhân sự phù hợp.</td></tr>`}</tbody></table></div>${listFooter(page, 'people', 'tài khoản')}</article>`;
};

const openCreateBeforePeople = openCreate;
openCreate = function (kind, id) {
  if (kind === 'new-person') {
    if (!needsRole('admin')) return toast('Chỉ quản trị viên mới được thêm tài khoản.');
    currentModalAction = kind;
    modal('Thêm nhân sự', [
      { name: 'id', label: 'Mã người dùng', required: true },
      { name: 'name', label: 'Họ và tên', required: true },
      { name: 'email', label: 'Email', type: 'email', required: true },
      { name: 'username', label: 'Tên đăng nhập', required: true },
      { name: 'password', label: 'Mật khẩu ban đầu', type: 'password', required: true },
      { name: 'role', label: 'Vai trò', options: personRoleOptions },
      { name: 'active', label: 'Trạng thái', options: [['true', 'Đang hoạt động'], ['false', 'Đã khóa']] }
    ], 'Thêm tài khoản');
    return;
  }
  if (kind === 'edit-person') {
    if (!needsRole('admin')) return toast('Chỉ quản trị viên mới được sửa tài khoản.');
    const person = people.find(row => row.id === id);
    if (!person) return toast('Không tìm thấy tài khoản cần sửa.');
    currentModalAction = kind;
    window.editingUserId = id;
    modal('Sửa thông tin nhân sự', [
      { name: 'name', label: 'Họ và tên', required: true },
      { name: 'email', label: 'Email', type: 'email', required: true },
      { name: 'username', label: 'Tên đăng nhập', required: true },
      { name: 'password', label: 'Mật khẩu mới', type: 'password', placeholder: 'Để trống nếu không đổi' },
      { name: 'role', label: 'Vai trò', options: personRoleOptions },
      { name: 'active', label: 'Trạng thái', options: [['true', 'Đang hoạt động'], ['false', 'Đã khóa']] }
    ], 'Lưu thay đổi');
    $('#modalForm [name="name"]').value = person.name || '';
    $('#modalForm [name="email"]').value = person.email || '';
    $('#modalForm [name="username"]').value = person.username || '';
    $('#modalForm [name="role"]').value = person.role || 'Employee';
    $('#modalForm [name="active"]').value = String(Boolean(person.active));
    return;
  }
  return openCreateBeforePeople(kind, id);
};

const submitModalBeforePeople = submitModal;
submitModal = async function (formData) {
  if (currentModalAction !== 'new-person' && currentModalAction !== 'edit-person') return submitModalBeforePeople(formData);
  if (!needsRole('admin')) throw new Error('Chỉ quản trị viên mới được quản lý tài khoản nhân sự.');
  const value = key => String(formData.get(key) || '').trim();
  const userId = currentModalAction === 'edit-person' ? window.editingUserId : value('id');
  const payload = {
    Manguoidung: userId,
    Tendangnhap: value('username'),
    matkhau: value('password'),
    Hoten: value('name'),
    Email: value('email'),
    Vaitro: value('role'),
    Trangthai: value('active') !== 'false'
  };
  await api(currentModalAction === 'new-person' ? apiPaths.users : `${apiPaths.users}/${encodeURIComponent(userId)}`, {
    method: currentModalAction === 'new-person' ? 'POST' : 'PUT',
    body: JSON.stringify(payload)
  });
  state.usersLoaded = false;
  state.users = [];
};

const bindDynamicBeforePeople = bindDynamic;
bindDynamic = function () {
  bindDynamicBeforePeople();
  document.querySelectorAll('[data-action="edit-person"]').forEach(button => {
    button.onclick = () => openCreate('edit-person', button.dataset.id);
  });
  document.querySelectorAll('[data-action="delete-person"]').forEach(button => {
    button.onclick = async () => {
      const id = button.dataset.id;
      if (!needsRole('admin')) return toast('Chỉ quản trị viên mới được xóa tài khoản.');
      if (id === state.userId) return toast('Không thể xóa tài khoản đang đăng nhập.');
      if (!confirm(`Xóa tài khoản ${id}? Thao tác này có thể bị từ chối nếu tài khoản đang được dữ liệu khác sử dụng.`)) return;
      button.disabled = true;
      try {
        await api(`${apiPaths.users}/${encodeURIComponent(id)}`, { method: 'DELETE' });
        state.usersLoaded = false;
        state.users = [];
        await loadDataFor('people');
        toast('Đã xóa tài khoản nhân sự.');
      } catch (error) {
        toast(error.message);
        button.disabled = false;
      }
    };
  });
};

function openAccountModal() {
  const person = state.users.find(user => user.id === state.userId) || people.find(user => user.id === state.userId);
  const name = person?.name || state.username || 'Chưa đăng nhập';
  const role = personRoleNames[person?.role || state.role] || person?.role || state.role || '—';
  const id = person?.id || state.userId || '—';
  const email = person?.email || 'Chưa cập nhật';
  $('#modal').innerHTML = `<div class="modal-header"><h2>Tài khoản của tôi</h2><button class="modal-close" data-action="close-modal" aria-label="Đóng">×</button></div><div class="account-summary"><span class="avatar avatar-purple">${escapeHTML(initials(name))}</span><div><b>${escapeHTML(name)}</b><small>${escapeHTML(role)}</small></div></div><dl class="account-details"><div><dt>Mã tài khoản</dt><dd>${escapeHTML(id)}</dd></div><div><dt>Tên đăng nhập</dt><dd>${escapeHTML(state.username || '—')}</dd></div><div><dt>Email</dt><dd>${escapeHTML(email)}</dd></div></dl><div class="modal-footer"><button type="button" class="btn btn-quiet" data-action="close-modal">Đóng</button>${state.token ? '<button type="button" class="btn btn-primary" id="accountSignOut">Đăng xuất</button>' : ''}</div>`;
  $('#modalBackdrop').classList.add('open');
  $('#modal').querySelectorAll('[data-action="close-modal"]').forEach(button => {
    button.addEventListener('click', () => $('#modalBackdrop').classList.remove('open'));
  });
  $('#accountSignOut')?.addEventListener('click', () => {
    $('#modalBackdrop').classList.remove('open');
    logout();
  });
}

function updateAccountButtons() {
  const person = state.users.find(user => user.id === state.userId) || people.find(user => user.id === state.userId);
  const name = person?.name || state.username || 'Tài khoản';
  const role = personRoleNames[person?.role || state.role] || person?.role || state.role || 'Đang đăng nhập';
  const initialsText = initials(name);
  if ($('#sidebarAccountName')) $('#sidebarAccountName').textContent = name;
  if ($('#sidebarAccountRole')) $('#sidebarAccountRole').textContent = role;
  if ($('#sidebarAccountAvatar')) $('#sidebarAccountAvatar').textContent = initialsText;
  if ($('#topAccountAvatar')) $('#topAccountAvatar').textContent = initialsText;
}

$('#profileButton')?.addEventListener('click', openAccountModal);
$('#accountButton')?.addEventListener('click', openAccountModal);

function applyRoleNavigation() {
  document.querySelectorAll('.nav-item[data-page]').forEach(item => {
    const visible = canAccessPage(item.dataset.page);
    item.hidden = !visible;
    item.style.display = visible ? '' : 'none';
  });
  document.querySelectorAll('.main-nav').forEach(nav => {
    const visible = [...nav.querySelectorAll('.nav-item')].some(item => !item.hidden);
    nav.hidden = !visible;
    nav.style.display = visible ? '' : 'none';
    const label = nav.previousElementSibling;
    if (label?.classList.contains('nav-label')) {
      label.hidden = !visible;
      label.style.display = visible ? '' : 'none';
    }
  });
}

$('#homeLogo')?.addEventListener('click', event => {
  event.preventDefault();
  navigate('dashboard');
});
const renderWithAccount = render;
render = function () {
  renderWithAccount();
  applyRoleNavigation();
  updateAccountButtons();
};
applyRoleNavigation();
updateAccountButtons();
